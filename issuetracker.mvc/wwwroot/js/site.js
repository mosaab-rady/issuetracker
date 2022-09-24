﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


let sidebar = document.getElementById("mySidenav");
let sidbarToggleBtn = document.getElementById("sidebar_toggle_btn");

sidbarToggleBtn.addEventListener("click", () => {
	toggle();
});

let hidden = false;
function toggle() {

	if (hidden) {
		openNav();
		hidden = !hidden;
	} else {
		closeNav();
		hidden = !hidden;
	}

}

function openNav() {
	document.getElementById("mySidenav").style.display = "block";
	document.getElementById("sidenvContainer").style.padding = "0";
}

function closeNav() {
	document.getElementById("mySidenav").style.display = "none";
	document.getElementById("sidenvContainer").style.padding = "0 1rem";
}