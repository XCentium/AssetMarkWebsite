$(document).ready(function () {
	genworthBreadcrumbUpdateLayout();

	$('#genworth-breadcrumb .genworth-breadcrumb-button').click(function (event) {
		if ($('#genworth-breadcrumb').hasClass('genworth-breadcrumb-expanded'))
			genworthBreadcrumbCollapse();
		else
			genworthBreadcrumbExpand();
	});

	// TODO: remove temporary preloading when something better is in place
	(new Image()).src = "/CMSContent/Images/breadcrumbAboutButtonOver.png";
	(new Image()).src = "/CMSContent/Images/breadcrumbCloseButton.png";
	(new Image()).src = "/CMSContent/Images/breadcrumbCloseButtonOver.png";
});

function genworthBreadcrumbExpand() {
	$('#genworth-breadcrumb .genworth-breadcrumb-collapsed-container').fadeOut('fast');
	$('#genworth-breadcrumb').removeClass('genworth-breadcrumb-collapsed').addClass('genworth-breadcrumb-expanded');
	$('#genworth-breadcrumb .genworth-breadcrumb-expanded-container').animate({ opacity: 'show', height: 'show' }, 'fast', function () {
	});
}

function genworthBreadcrumbCollapse() {
	$('#genworth-breadcrumb .genworth-breadcrumb-expanded-container').animate({ opacity: 'hide', height: 'hide' }, 'fast');
	$('#genworth-breadcrumb .genworth-breadcrumb-collapsed-container').fadeIn('fast');
	$('#genworth-breadcrumb').removeClass('genworth-breadcrumb-expanded').addClass('genworth-breadcrumb-collapsed');
}

function genworthBreadcrumbUpdateLayout() {
	var expandedContainer = $('#genworth-breadcrumb .genworth-breadcrumb-expanded-container');
	var title = $('#genworth-breadcrumb .genworth-breadcrumb-intro-title');
	var titleContainer = $('#genworth-breadcrumb .genworth-breadcrumb-intro-title-container');

	// if container is hidden, momentarily show it so the browser calculates the proper dimensions.
	var hide = false;
	if (expandedContainer.is(':hidden')) {
		expandedContainer.show();
		hide = true;
	}

	var titleWidth = title.width();
	var titleContainerOuterWidth = titleContainer.outerWidth();
	var titleContainerWidth = titleContainer.width();

	if (hide)
		expandedContainer.hide();

	if (titleWidth <= 0)
		return;

	var titleContainerMinWidth = parseInt(titleContainer.css('minWidth'));
	if (isNaN(titleContainerMinWidth))
		titleContainerMinWidth = 0;

	if (titleContainerMinWidth > 0 && titleWidth < titleContainerMinWidth)
		titleWidth = titleContainerMinWidth;

	var introContainer = $('#genworth-breadcrumb .genworth-breadcrumb-intro-container');

	titleContainer.css({ marginRight: -titleContainerOuterWidth });
	introContainer.css({ marginLeft: titleContainerWidth });
}
