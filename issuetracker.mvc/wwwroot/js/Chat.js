var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (commentText, date, username, image) {
	var newComment = `<div class="my-2 border p-3 rounded-3" style="background-color: #ebebeb;">
					<div class="d-flex align-items-center">
						<img src="/img/${image}" class="rounded-circle boxshadow me-3" width="30px" height="30px" alt="">
						<div class="d-flex gap-3">
							<p class="text-muted mb-0">${username}
								<span class="ms-2 mb-0" style="font-size: 0.8rem;">
									${date}
								</span>
							</p>
						</div>
					</div>
					<div class="px-5 mt-2">
					<p class="text-dark mb-0">
					${commentText}
					</p>
					</div>
				</div>`

	document.getElementById("CommentsDiv").insertAdjacentHTML("beforeend", newComment);
	document.getElementById("CommentsDiv").scrollTop = document.getElementById("CommentsDiv").scrollHeight;

});

connection.start().then(function () {
	var groupName = document.getElementById("groupNameInput").value;
	connection.invoke("JoinGroup", groupName);
}).catch(function (err) {
	return console.error(err.toString());
});




