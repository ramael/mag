define(['knockout', 'sammy', 'text!./m-home.html'], function (ko, sammy, tplString) {

    function HomeModel(params) {
        const self = this;
        self.root = params.value;
        self.selectedPage = ko.observable();

        // Events
        self.logout = function () {
            self.root.user("");
        };

        self.selectPage = function (page) {
            location.hash = page.id;
        };

        // Client side routes
        sammy(function () {
            this.get('#:page', function () {
                const pageid = this.params['page'];
                const page = self.root.pages.find(function (p) { return p.id === pageid });
                self.selectedPage(page);
            });

            this.get('#:page/:itemid', function () {
                const pageid = this.params['page'];
                const page = self.root.pages.find(function (p) { return p.id === pageid });
                page.itemid = this.params['itemid'];
                self.selectedPage(page);
            });

            this.get('#:page/:parentitemid/:itemid', function () {
                const pageid = this.params['page'];
                const page = self.root.pages.find(function (p) { return p.id === pageid });
                page.parentitemid = this.params['parentitemid'];
                page.itemid = this.params['itemid'];
                self.selectedPage(page);
            });

            // Default route
            this.get('', function () {
                this.app.runRoute('get', '#loadedcarts');
            });

        }).run();

    }

    return { viewModel: HomeModel, template: tplString };

});
