/// <reference path="libs/jquery.d.ts" />

module Helpers {
	export function addSizeHandler(handler: () => void) {
		$(handler);
		$(window).on("resize", handler);
	}
};