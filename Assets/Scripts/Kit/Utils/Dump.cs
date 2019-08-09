using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public delegate void LogDelegate(object message);

public class Output
{   
    private class Dumper
    {
        private readonly StringBuilder stringBuilder;
        public int level{get;set;}
        public string LineBreakChar = Environment.NewLine;
        public int maxLevel = int.MaxValue;
        public int indentSize = 4;
        public char indentChar = ' ';
        public Dumper(){
            this.level = 0;
            this.stringBuilder = new StringBuilder();
        }

        public bool isMaxLevel(){
            return this.level > this.maxLevel;
        }

        private string calculateSpace(char c, int level, int size){
            var space = new string(c, level * size);
            return space;
        }

        public void Write(string value, int? intentLevel = null){ //int? 可空类型
            var space = calculateSpace(this.indentChar, intentLevel ?? 0, this.indentSize);  // 使用var关键字 必须在定义的时候初始化 编译器自动判断类型 和使用强类型方式定义变量效率一样
            this.stringBuilder.Append(space + value);
        }

        public string GetVariableName(object element)
        {
            if (element == null)
            {
                return "var";
            }

            var type = element.GetType();
            var variableName = type.Name;
            return ToLowerFirst(variableName);
        }

        private string ToLowerFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            var a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }

        private void LineBreak(){
            this.stringBuilder.Append(this.LineBreakChar);
        }

        private void StartLine(string value, int? level = null){ // mark
            var space = calculateSpace(this.indentChar, level ?? this.level, this.indentSize);
            this.stringBuilder.Append(space + value);
        }

        private static readonly Dictionary<string, string> EscapeMapping = new Dictionary<string, string>(){
            { "\'", "\\\'" }, //  Single quote
            { "\"", "\\\"" }, //  Double quote
            { "\\\\", @"\\" }, // Backslash
            { "\a", @"\a" }, // Alert (ASCII 7)
            { "\b", @"\b" }, // Backspace (ASCII 8)
            { "\f", @"\f" }, // Form feed (ASCII 12)
            { "\n", @"\n" }, // New line (ASCII 10)
            { "\r", @"\r" }, // Carriage return (ASCII 13)
            { "\t", @"\t" }, // Horizontal tab (ASCII 9)
            { "\v", @"\v" }, // Vertical quote (ASCII 11)
            { "\0", @"\0" }, // Empty space (ASCII 0)
        };
        private static readonly Regex EscapeRegex = new Regex(string.Join("|", EscapeMapping.Keys));
        public static string Escape(string s){ // mark
            return EscapeRegex.Replace(s, EscapeMatchEval);
        }
        private static string EscapeMatchEval(Match m)
        {
            if (EscapeMapping.ContainsKey(m.Value))
            {
                return EscapeMapping[m.Value];
            }

            return EscapeMapping[Regex.Escape(m.Value)];
        }

        public void FormatValue(object obj, int indentLevel, int? startBracketLevel = null){
            if (this.isMaxLevel())
                return;
            if (obj == null){
                this.Write("null", indentLevel);
                return;
            }
            if (obj is bool){ // is判断对象是否为某一类型
                this.Write($"{obj.ToString().ToLower()}", indentLevel);
                return;
            }

            if(obj is string){
                var str = Escape($@"{obj}");  //@加在字符串前表示其中的转义字符不被处理 可以让字符串跨行 可以使关键字作为标识符
                this.Write($"\"{str}\"", indentLevel);
                return;
            }

            if(obj is char){
                var c = obj.ToString().Replace("\0", "").Trim();
                this.Write($"\'{c}\'", indentLevel);
                return;
            }

            if(obj is double){
                this.Write($"{obj}d", indentLevel);
                return;
            }

            if(obj is decimal){ // decimal 128位精确的十进制值
                this.Write($"{obj}m", indentLevel);
                return;
            }

            // 8位无符号整数 8位有符号整数 32位有符号整数 32位无符号整数 16位有符号整数 16位无符号整数
            if(obj is byte || obj is sbyte || obj is int || obj is uint || obj is short || obj is ushort){ 
                this.Write($"{obj}", indentLevel);
                return;
            }

            if (obj is float)
            {
                this.Write($"{obj}f", indentLevel);
                return;
            }

            if (obj is long || obj is ulong) // 64位有符号整数 64位无符号整数
            {
                this.Write($"{obj}L", indentLevel);
                return;
            }

            if (obj is DateTime dateTime){

            }

            if (obj is Enum){

            }

            if (obj is Guid guid){

            }

            if (obj is Array){
                this.ForamtArray((Array)obj, startBracketLevel);
                return;
            }

            
            Type type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)){ // GetGenericTypeDefinition获取当前构造类型的基础泛型类定义 typeof的参数必须是类
                object key = obj.GetType().GetProperty(nameof(KeyValuePair<object,object>.Key)).GetValue(obj);
                object value = obj.GetType().GetProperty(nameof(KeyValuePair<object,object>.Value)).GetValue(obj);
                this.FormatValue(key, this.level);
                this.Write(" : ");
                this.FormatValue(value, 0, 0);
                return;
            }


            if (obj is IEnumerable){ // 迭代器
                this.StartLine("{", startBracketLevel);
                this.LineBreak();
                this.WriteItems((IEnumerable)obj);
                this.StartLine("}");
                return;
            }

            this.CreateObject(obj, indentLevel);

        }

        private void ForamtArray(Array array, int? startBracketLevel = null){
            int rank = array.Rank;
            int count = 0;
            int[] breakPoints = new int[rank];
            breakPoints[rank - 1] = array.GetLength(rank - 1);
            for(int i = rank - 2; i >= 0; i --){
                breakPoints[i] = array.GetLength(i) * breakPoints[i + 1];
            }
            IEnumerator enumerator = array.GetEnumerator();
            while(enumerator.MoveNext()){
                count ++;
                bool startWithBracket = false;
                for(int i = 0; i < breakPoints.Length; i ++){ 
                    if ((count - 1) % breakPoints[i] == 0){
                        startWithBracket = true;
                        var usestartBracketLevel = count == 1 ? startBracketLevel : null;
                        this.WriteStartBracket(breakPoints.Length - i, count != 1, usestartBracketLevel);
                        break;
                    }
                }
                
                if (!startWithBracket){
                    this.Write(",");
                    this.LineBreak();
                }
                this.FormatValue(enumerator.Current, this.level);

                for(int i = rank - 1; i >= 0; i --){
                    if (count % breakPoints[i] == 0){
                        this.LineBreak();
                        this.level --;
                        this.StartLine("}");
                    }
                }
            }
        }

        private void WriteStartBracket(int count, bool startWithComma, int? startBracketLevel = null){
            for(int i = 0; i < count; i ++){
                if (startWithComma && i == 0){
                    this.Write(",");
                    this.LineBreak();
                }
                this.StartLine("{", i == 0 ? startBracketLevel : null);
                this.LineBreak();
                this.level ++;
            }
        }

        private void WriteItems(IEnumerable items){
            this.level ++;
            if (this.isMaxLevel()){
                this.level --;
                return;
            }

            var enumer = items.GetEnumerator();
            if (enumer.MoveNext()){
                this.FormatValue(enumer.Current, this.level);
                while(enumer.MoveNext()){
                    this.Write(",");
                    this.LineBreak();
                    this.FormatValue(enumer.Current, this.level);
                }
                this.LineBreak();
            }
            this.level --;
        }

        private void CreateObject(object obj, int? indentLevel = null){

        }

        public override string ToString(){
            return this.stringBuilder.ToString();
        }
    }

    public static LogDelegate Log = new LogDelegate(UnityEngine.Debug.Log);

    // public static void Dump(object obj){
    //     Dump(obj, "<var>");
    // }

    public static string Dump(object obj, string msg = null){
        var dumper = new Dumper();
        msg = msg ?? $"{dumper.GetVariableName(obj)}";
        dumper.Write((msg) + " = ");  //$是为了替代string.format() 原先赋值需要占位符和变量 现在可以把字符串中的变量用{}包含起来以达到识别c#变量的目的
        dumper.FormatValue(obj, 0);
        dumper.Write(";");
        // // Log(obj.GetType().name);
        // if (obj is Array){
        //     string output = "{";
        //     Array arr = (Array)obj;
        //     foreach (object a in arr)
        //     {
        //         output += (a + ",");
        //     }
        //     output += "}";
        //     Log(msg + " : " + output);
        // }else{
        //     Log(msg + " : " + obj);
        // }
        string str = dumper.ToString();
        Log(str);
        return str;
    } 
}



