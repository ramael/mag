define(['knockout', 'text!./m-home.html'], function (ko, tplString) {

    function Page(id, label, component) {
        this.id = id;
        this.label = label;
        this.component = component;
    }

    function HomeModel(params) {
        const self = this;
        self.root = params.value;
        self.selectedPage = ko.observable();
        self.pages = [
            new Page("components", "Components", "m-components"),
            new Page("loadedcarts", "Loaded Carts", "m-loadedcarts"),
            new Page("currentstock", "Current Stock", "m-currentstock")
        ];

        // Events
        self.logout = function () {
            self.root.user("");
        };

        self.selectPage = function (page) {
            self.selectedPage(page);
        };

        // Init
        self.selectPage(self.pages[2]);
    }

    return { viewModel: HomeModel, template: tplString };

});
