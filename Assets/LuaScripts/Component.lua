print("Load Behavior.lua")

local Component = {}

function Component:Start()
    dump(self, "self......")
    print("start............")
end

function Component:Update()
    print("update............")
end

return Component