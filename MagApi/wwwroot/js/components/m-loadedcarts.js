define(['knockout', 'text!./m-loadedcarts.html'], function (ko, tplString) {

    function LoadedCartsModel(params) {
        const self = this;
        self.root = params.value;
    }

    return { viewModel: LoadedCartsModel, template: tplString };

});
