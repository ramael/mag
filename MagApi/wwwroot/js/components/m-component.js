define(['knockout', 'text!./m-component.html'], function (ko, tplString) {

    function ComponentModel(params) {
        const self = this;
        self.root = params.value;
        self.id = params.data;
        self.modal = params.modal;

        self.component = ko.observable();

        // Events
        self.isEdit = function () {
            return self.id !== -1;
        };
        self.doSubmit = function () {
            if (self.isEdit()) {
                self.updateComponent();
            } else {
                self.createComponent();
            }
        };
        self.doCancel = function () {
            if (self.modal) {
                self.root.modalComponentStatus("cancel");
            }
        };

        // Ajax
        self.getComponent = function (id) {
            $.ajax({
                url: self.root.apis.component.replace("{id}", id),
                type: 'get',
                contentType: 'application/json',
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                if (data) {
                    self.component(new self.root.contracts.component(data.id, data.code, data.description, data.notes));
                }
            }).fail(function (data) {
                console.error("component error", data);
                self.component("");
                self.root.handleServerError(data);
            });
        };

        self.createComponent = function () {
            const c = new self.root.contracts.component(self.component().id, self.component().code, self.component().description, self.component().notes);
            $.ajax({
                url: self.root.apis.components,
                type: 'post',
                contentType: 'application/json',
                data: JSON.stringify(c),
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                self.component("");
                if (self.modal) {
                    self.root.modalComponentStatus("ok");
                }
            }).fail(function (data) {
                console.error("create component error", data);
                self.component("");
                self.root.handleServerError(data);
            });
        };

        self.updateComponent = function () {
            const c = new self.root.contracts.component(self.component().id, self.component().code, self.component().description, self.component().notes);
            $.ajax({
                url: self.root.apis.component.replace("{id}", self.id),
                type: 'put',
                contentType: 'application/json',
                data: JSON.stringify(c),
                processData: false,
                headers: {
                    'Authorization': 'Bearer ' + self.root.user().token
                }
            }).done(function (data) {
                self.component("");
                if (self.modal) {
                    self.root.modalComponentStatus("ok");
                }
            }).fail(function (data) {
                console.error("update component error", data);
                self.component("");
                self.root.handleServerError(data);
            });
        };

        // Init
        self.component(new self.root.contracts.component(0, "", "", ""))
        if (self.isEdit()) {
            self.getComponent(self.id);
        }
    }

    return { viewModel: ComponentModel, template: tplString };

});
