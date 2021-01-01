require.config({
    baseUrl: ".",
    paths: {
        "text": "../libs/requirejs/text.min",
        "jquery": "../libs/jquery/jquery.min",
        "knockout": "../libs/knockout/knockout-latest.min",
        "bootstrap": "../libs/bootstrap/js/bootstrap.bundle.min",
        "sammy": "../libs/sammy/sammy.min",
    }
});

require(['text', 'jquery', 'knockout', 'bootstrap', 'js/contracts/apis', 'js/contracts/contracts', 'js/contracts/pages'], function (text, $, ko, bs, apis, contracts, pages) {

    function MagModel() {
        this.apis = apis;
        this.contracts = contracts;
        this.pages = pages;
        this.user = ko.observable();
    }

    $(document).ready(function () {
        ko.components.register('m-login', { require: 'js/components/m-login' });
        ko.components.register('m-home', { require: 'js/components/m-home' });
        for (let i = 0; i < pages.length; i++) {
            ko.components.register(pages[i].component, { require: 'js/components/' + pages[i].component });
        };

        ko.applyBindings(new MagModel());
    });
});
