print("Load Component.lua")

local Component = {}

function Component:Awake()
    print("awake............." .. tostring(self) .. "  " .. tostring(self.gameObject))
    self.gameObject:GetComponent("Button").onClick:AddListener(self.aaa)
end

function Component:Start()
    print("start............." .. tostring(self) .. "  " .. tostring(self.gameObject))
end

function Component:aaa()
    print("click aaa")
end

function Component:Update()
    
end

function Component:OnDestroy()
    -- 点击按钮后依然会报XLua Dispose exception : try to dispose a LuaEnv with C# callback!
    self.gameObject:GetComponent("Button").onClick:RemoveListener(self.aaa)
end

return Component