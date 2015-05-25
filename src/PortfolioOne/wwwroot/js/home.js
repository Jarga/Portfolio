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

        self.LeftPortfolioLinks = ko.observableArray(initialModelData.LeftPortfolioLinks);
        self.RightPortfolioLinks = ko.observableArray(initialModelData.RightPortfolioLinks);
        self.TimelineEvents = ko.observableArray(initialModelData.TimelineEvents);
        self.MySkills = ko.observableArray(initialModelData.MySkills);
        self.MyWorkExamples = ko.observableArray(initialModelData.MyWorkExamples);
    };

    var viewModel = new HomeModel({
        LeftPortfolioLinks:
            [
               { name: "linkedin", link: "https://www.linkedin.com/pub/sean-mcadams/29/448/bb3" },
               { name: "stack-exchange", link: "https://careers.stackoverflow.com/seanmcadams" },
               { name: "github", link: "https://github.com/Jarga?tab=repositories" }
            ],
        RightPortfolioLinks:
            [
               { name: "stack-overflow", link: "http://stackoverflow.com/users/3900634/jarga" },
               { name: "jsfiddle", link: "http://jsfiddle.net/user/Jarga/fiddles/" },
               { name: "docker", link: "https://hub.docker.com/u/jarga/" }
            ],
        TimelineEvents:
            [
               { year: "2010", event: "Computer Science Bachelors" },
               { year: "2010", event: "Full-time at Lender Processing Services" },
               { year: "2012", event: "Speaker at IBM Innovate" },
               { year: "2012", event: "Full-time at EverBank" },
               { year: "2014", event: "Full-time at OceansideTen" },
               { year: "2014", event: "Contract with Saint John Solutions" },
               { year: "2015", event: "Masters in Software Engineering" },
            ],
        MySkills:
            [
               { icon: "sitemap", skill: "Architecture" },
               { icon: "lock", skill: "Security" },
               { icon: "code", skill: "Development" },
               { icon: "comments", skill: "Comunication" },
               { icon: "fighter-jet", skill: "Performance" },
               { icon: "globe", skill: "Globalization" }
            ],
        MyWorkExamples:
            [
               { icon: "cogs", title: "Automation Frameworks", description: "I have an extensive background in automated UI testing and have written frameworks to simplify this process." },
               { icon: "university", title: "ASP.NET 5 Beta", description: "I have high hopes for the next ASP.NET release and have been following it for a while, in fact this site is running on ASP.NET 5 and Docker." },
               { icon: "group", title: "Web Design and Usability", description: "I am experienced working with designers and other developers to deliver an efficient, clean, and usable application." },
               { icon: "beer", title: "Soft Skills", description: "I am easy to get along with and work well with others, I am a proffesional when it comes to interacting with clients." },
            ],

    });

    ko.applyBindings(viewModel);

    $(document).on('click', '#send_contact_message', function (event) {
        var me = $(this);
        var form = $('section#contact').find('form');
        form.mask("Loading...");
        $.ajax({
            method: 'POST',
            url: me.data('post-url'),
            data: form.serialize(),
            success: function (result) {
                if (result.Success) {
                    // remove errors and display success
                    form.find('.field-validation-error').remove();
                    displayContactAlert('alert-success', result.Message);
                } else {
                    //Model Error
                    form.replaceWith(result);
                }
            },
            error: function () {
                // Fail message
                displayContactAlert('alert-danger', 'Sorry, it seems that my mail server is not responding. Please try again later!');
            },
            complete: function () {
                form.unmask();
            }
        });
        event.preventDefault();
    })
});

function displayContactAlert(alertClass, message) {
    $('.contact-alert').html("<div class='alert " + alertClass + "'>");
    $('.contact-alert > .' + alertClass).html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'><i class='fa fa-times'></i></button><strong>" + message + "</strong></div>");
    $('html, body').animate({ scrollTop: $('.contact-alert').offset().top - ( $('.navbar-fixed-top').outerHeight() + 5) }, 500);
}
