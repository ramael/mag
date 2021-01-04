define([], function () {

    function Page(id, label, component, menu) {
        this.id = id;
        this.label = label;
        this.component = component;
        this.menu = menu;
        this.parentitemid = null;
        this.itemid = null;
    }

    return [
        new Page("component", "Component", "m-component", false),
        new Page("components", "Components", "m-components", true),
        new Page("loadedcart", "Loaded Cart", "m-loadedcart", false),
        new Page("loadedcartdetail", "Loaded Cart Details", "m-loadedcartdetail", false),
        new Page("loadedcarts", "Loaded Carts", "m-loadedcarts", true),
        new Page("currentstock", "Current Stock", "m-currentstock", true)
    ]
});
