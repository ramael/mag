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

require(['text', 'jquery', 'knockout', 'bootstrap', 'sammy', 'js/contracts/apis', 'js/contracts/contracts'], function (text, $, ko, bs, sm, apis, contracts) {

    function MagModel() {
        this.apis = apis;
        this.contracts = contracts;
        this.user = ko.observable();
    }

    $(document).ready(function () {
        ko.components.register('m-login', { require: 'js/components/m-login' });
        ko.components.register('m-home', { require: 'js/components/m-home' });
        ko.components.register('m-components', { require: 'js/components/m-components' });
        ko.components.register('m-loadedcarts', { require: 'js/components/m-loadedcarts' });
        ko.components.register('m-currentstock', { require: 'js/components/m-currentstock' });
        
        ko.applyBindings(new MagModel());
    });
});
