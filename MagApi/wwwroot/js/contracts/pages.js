define([], function () {

    function Page(id, label, component) {
        this.id = id;
        this.label = label;
        this.component = component;
    }

    return [
            new Page("components", "Components", "m-components"),
            new Page("loadedcarts", "Loaded Carts", "m-loadedcarts"),
            new Page("currentstock", "Current Stock", "m-currentstock")
    ]
});
