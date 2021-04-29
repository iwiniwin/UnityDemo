require("LuaKit.core.object")
local CreateComponent = function(componentPath)
    return new(require(componentPath))
end

return CreateComponent