define(['knockout', 'text!./m-login.html'], function (ko, tplString) {

    function LoginModel(params) {
        const self = this;
        self.root = params.value;
        self.error = ko.observable(false);
        self.errorMessage = ko.observable("");
        self.username = "";
        self.password = "";

        // Events
        self.login = function () {
            const lr = new self.root.contracts.loginrequest(this.username, this.password);
            self.root.apis.doLogin(lr)
                .done(function (user) {
                    self.root.user(user)
                    self.error(false);
                    self.errorMessage("");
                }).fail(function (data) {
                    self.error(true);
                    self.errorMessage(data.status + " - " + data.responseText);
                });
        };
    }

    return { viewModel: LoginModel, template: tplString };

});
