define(['knockout', 'text!./m-components.html'], function (ko, tplString) {

    function ComponentsModel(params) {
        const self = this;
        self.root = params.value;
    }

    return { viewModel: ComponentsModel, template: tplString };

});
