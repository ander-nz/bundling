/// <reference path="helpers.ts" />
/// <reference path="libs/jquery.d.ts" />

module App {
	Helpers.addSizeHandler(() => {
		var fontSize = ~~($(window).width() / 80);
		$("html").css({ "font-size": fontSize });
	});
};