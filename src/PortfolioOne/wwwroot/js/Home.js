ko.bindingHandlers.href = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.attr.update(element, function () {
            return { href: valueAccessor() }
        });
    }
};

$(document).ready(function () {
    var HomeModel = function(initialModelData) {
        var self = this;

        self.PortfolioLinks = ko.observableArray(initialModelData.portfolioLinks);
    };

    var viewModel = new HomeModel({
        portfolioLinks:
            [
               { name: "LinkedIn", link: "https://www.linkedin.com/pub/sean-mcadams/29/448/bb3" },
               { name: "StackOverflow Carrers", link: "https://careers.stackoverflow.com/seanmcadams" },
               { name: "Github", link: "https://github.com/Jarga?tab=repositories" },
               { name: "StackOverflow", link: "http://stackoverflow.com/users/3900634/jarga" },
               { name: "JsFiddle", link: "http://jsfiddle.net/user/Jarga/fiddles/" }
            ]
    });

    ko.applyBindings(viewModel);
});