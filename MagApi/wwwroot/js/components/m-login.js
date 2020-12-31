define(['knockout', 'text!./m-login.html'], function (ko, tplString) {

    function LoginModel(params) {
        const self = this;
        self.root = params.value;
        self.error = ko.observable(false);
        self.errorMessage = ko.observable("");
        self.username = "";
        self.password = "";
        self.login = function () {
            const lr = new self.root.contracts.loginrequest(this.username, this.password);
            $.ajax({
                url: self.root.apis.login,
                dataType: 'json',
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify(lr),
                processData: false
            }).done(function (data) {
                const user = new self.root.contracts.loginresponse(data.username, data.firstname, data.lastname, data.roles, data.token);
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
