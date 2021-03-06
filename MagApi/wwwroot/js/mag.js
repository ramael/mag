﻿require.config({
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
        const self = this;
        self.apis = apis;
        self.contracts = contracts;
        self.pages = pages;
        self.user = ko.observable();
        //debug
        //self.user(new contracts.loginresponse("t1", "Test1", "T1LN", ["WarehouseManager"], "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InQyIiwicm9sZSI6IkNhcnRNYW5hZ2VyIiwibmJmIjoxNjA5ODQ4Mjc4LCJleHAiOjE2MDk4NTAwNzgsImlhdCI6MTYwOTg0ODI3OCwiaXNzIjoiaHR0cDovL21hZy5vcmciLCJhdWQiOiJodHRwOi8vbWFnLm9yZyJ9.6rRWjJJFVfc9SctAE1a5DIOl6dXaQgThC6klZNbAsC0"));

        self.modalMessage = ko.observable();
        self.modalConfirm = ko.observable();
        self.modalComponent = ko.observable();
        self.modalComponentStatus = ko.observable("");
        self.modalComponentStatus.subscribe(function (status) {
                                    if (!status) return;
                                    if (status && status !== "cancel") {
                                        const callback = self.modalComponent().callback;
                                        if (callback) {
                                            callback(status);
                                        }
                                    }

                                    if (self.modalComponentDialog) {
                                        self.modalComponentDialog.modal("hide");
                                    }
                                });

        self.modalMessageDialog = null;
        self.modalConfirmDialog = null;
        self.modalComponentDialog = null;

        self.showModalMessage = function (component) {
            if (self.modalMessageDialog) {
                console.warn("Only one message dialog allowed.");
                return;
            }

            self.modalMessage(component);
            self.modalMessageDialog = $("#modalMessage").modal({ show: true, focus: true })
                                                        .on("hidden.bs.modal", function (e) {
                                                            self.modalMessage("");
                                                            self.modalMessageDialog.modal("dispose");
                                                            self.modalMessageDialog = null;
                                                        });
        };

        self.showModalConfirm = function (component) {
            if (self.modalConfirmDialog) {
                console.warn("Only one confirm dialog allowed.");
                return;
            }

            self.modalConfirm(component);
            self.modalConfirmDialog = $("#modalConfirm").modal({ show: true, focus: true })
                                                        .on("hidden.bs.modal", function (e) {
                                                            self.modalConfirm("");
                                                            self.modalConfirmDialog.modal("dispose");
                                                            self.modalConfirmDialog = null;
                                                        });
        };
        self.confirmModalConfirm = function () {
            const callback = self.modalConfirm().callback;
            if (callback) {
                callback(self.modalConfirm().data);
            }
            if (self.modalConfirmDialog) {
                self.modalConfirmDialog.modal("hide");
            }
        };

        self.showModalComponent = function (component) {
            if (self.modalComponentDialog) {
                console.warn("Only one component dialog allowed.");
                return;

            }
            self.modalComponent(component);
            self.modalComponentDialog = $("#modalComponent").modal({ show: true, focus: true })
                                                            .on("hidden.bs.modal", function (e) {
                                                                self.modalComponent("");
                                                                self.modalComponentStatus("");
                                                                self.modalComponentDialog.modal("dispose");
                                                                self.modalComponentDialog = null;
                                                            });
        };

        self.handleServerError = function (data) {
            switch (data.status) {
                case 400:
                    self.showModalMessage(new self.contracts.modalMessage("Bad request", data.responseJSON.title));
                    break;
                case 401:
                    self.user("");
                    break;
                case 403:
                    self.showModalMessage(new self.contracts.modalMessage("Security", "The access to the required resource is forbidden"));
                    break;
                case 409:
                    const msg = (data.responseText ? data.responseText : "A duplicate resource already exists");
                    self.showModalMessage(new self.contracts.modalMessage("Conflict", msg));
                    break;
                default:
                    self.showModalMessage(new self.contracts.modalMessage("Server", data.responseText));
                    break;
            }
        }
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
