Shader "Unlit/BumpedSpecular"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)  // 控制物体的整体色调
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bump Map", 2D) = "bump" {}
        _Specular ("Specular Color", Color) = (1, 1, 1, 1)  // 控制材质的高光反射颜色
        _Gloss ("Gloss", Range(8.0, 256)) = 20  // 控制高光区域的大小
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags {"LightMode"="ForwardBase"}  // 使用前向渲染路径中的ForwardBase路径

            CGPROGRAM

            /*
            编译指令，保证Unity为相应类型的Pass生成所有需要的Shader变种，这些变种会处理不同
            条件下的渲染逻辑，同时Unity也会在背后声明相关的内置变量并传递到Shader中
            */
            #pragma multi_compile_fwdbase  

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"  // 计算阴影所使用的宏在这个文件中声明

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;  // 使用纹理名_ST来声明某个纹理的属性，ST是缩放和平移的缩写
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float4 _Specular;
            float _Gloss;

            // 顶点着色器输入结构体
            struct a2v
            {
                float4 vertex : POSITION;       // 顶点坐标
                float3 normal : NORMAL;         // 顶点法线
                float4 tangent : TANGENT;       // 顶点切线
                float4 texcoord : TEXCOORD0;    // 第一组纹理坐标
            };

            // 顶点着色器输出结构体
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0: TEXCOORD1;
                float4 TtoW1: TEXCOORD2;
                float4 TtoW2: TEXCOORD3;
                SHADOW_COORDS(4)  // 声明一个用于对阴影纹理采样的坐标，这个宏的参数是下一个可用的插值寄存器索引值
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // 计算经过平铺和平移后的纹理坐标
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                // 在世界空间下进行光照计算
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // w分量决定binormal的方向
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                /*
                由于一个插值寄存器最多只能存储float4大小的变量
                因此将从切线空间到世界空间的变换矩阵按行拆成多个变量存储
                */
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                // 计算并向片元着色器传递阴影坐标
                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                // 使用UnpackNormal函数对法线纹理进行采用和解码
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                // 通过点乘模拟矩阵乘法，实现将法线从切线空间变换到世界空间
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));

                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo.rgb * saturate(dot(bump, lightDir));

                fixed3 halfDir = normalize(viewDir + lightDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(halfDir, bump)), _Gloss);

                // 用于统一计算光照衰减和阴影，会将光照衰减和阴影值相乘后存储到第一个参数中
                UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                return fixed4(ambient + (diffuse + specular) * atten, 1.0);
            }
            ENDCG
        }
        Pass
        {
            Tags {"LightMode"="ForwardAdd"}  // 使用前向渲染路径中的ForwardAdd路径

            // 开启和设置混合模式，希望Additional Pass计算得到的光照结果可以在帧缓存中与之前的光照叠加
            Blend One One

            CGPROGRAM

            /*
            保证在Additional Pass中访问到正确的光照变量
            */
            #pragma multi_compile_fwdadd 

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"  // 计算阴影所使用的宏在这个文件中声明

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;  // 使用纹理名_ST来声明某个纹理的属性，ST是缩放和平移的缩写
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            fixed4 _Specular;
			float _Gloss;

            // 顶点着色器输入结构体
            struct a2v
            {
                float4 vertex : POSITION;       // 顶点坐标
                float3 normal : NORMAL;         // 顶点法线
                float4 tangent : TANGENT;       // 顶点切线
                float4 texcoord : TEXCOORD0;    // 第一组纹理坐标
            };

            // 顶点着色器输出结构体
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0: TEXCOORD1;
                float4 TtoW1: TEXCOORD2;
                float4 TtoW2: TEXCOORD3;
                SHADOW_COORDS(4)  // 声明一个用于对阴影纹理采样的坐标，这个宏的参数是下一个可用的插值寄存器索引值
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // 计算经过平铺和平移后的纹理坐标
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

                // 在世界空间下进行光照计算
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // w分量决定binormal的方向
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

                /*
                由于一个插值寄存器最多只能存储float4大小的变量
                因此将从切线空间到世界空间的变换矩阵按行拆成多个变量存储
                */
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

                // 计算并向片元着色器传递阴影坐标
                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                // 使用UnpackNormal函数对法线纹理进行采用和解码
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                // 通过点乘模拟矩阵乘法，实现将法线从切线空间变换到世界空间
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));

                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;

                fixed3 diffuse = _LightColor0.rgb * albedo.rgb * saturate(dot(bump, lightDir));

                fixed3 halfDir = normalize(viewDir + lightDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(halfDir, bump)), _Gloss);

                // 用于统一计算光照衰减和阴影，会将光照衰减和阴影值相乘后存储到第一个参数中
                UNITY_LIGHT_ATTENUATION(atten, i, worldPos);

                // Additional Pass中不再计算环境光
                return fixed4((diffuse + specular) * atten, 1.0);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
