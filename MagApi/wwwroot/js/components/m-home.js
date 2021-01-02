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

            // Default route
            this.get('', function () {
                this.app.runRoute('get', '#components');
            });

        }).run();

    }

    return { viewModel: HomeModel, template: tplString };

});
