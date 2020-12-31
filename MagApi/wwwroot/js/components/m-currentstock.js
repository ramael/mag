define(['knockout', 'text!./m-currentstock.html'], function (ko, tplString) {

    function CurrentStockModel(params) {
        const self = this;
        self.root = params.value;
    }

    return { viewModel: CurrentStockModel, template: tplString };

});
