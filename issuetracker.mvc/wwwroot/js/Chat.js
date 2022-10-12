var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();



//Disable the send button until connection is established.
// document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (commentText, date, username, image) {
	var newComment = `<div class="my-2 bg-dark p-3 rounded-3">
					<div class="d-flex align-items-center">
						<img src="/img/${image}" class="rounded-circle boxshadow me-3" width="30px" height="30px" alt="">
						<div class="d-flex gap-3">
							<p class="text-light mb-0">${username}
								<span class="ms-2 mb-0" style="font-size: 0.8rem;">
									${date}
								</span>
							</p>
						</div>
					</div>
					<div class="px-5 mt-2">
					
							<p class="text-muted mb-0">
								${commentText}
							</p>
						
					</div>
				</div>`

	// var li = document.createElement("li");


	document.getElementById("CommentsDiv").insertAdjacentHTML("beforeend", newComment);
	document.getElementById("CommentsDiv").scrollTop = document.getElementById("CommentsDiv").scrollHeight;
	// We can assign user-supplied strings to an element's textContent because it
	// is not interpreted as markup. If you're assigning in any other way, you 
	// should be aware of possible script injection concerns.
	// li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
	// document.getElementById("sendButton").disabled = false;
	var groupName = document.getElementById("groupNameInput").value;
	connection.invoke("JoinGroup", groupName);
}).catch(function (err) {
	return console.error(err.toString());
});




// document.getElementById("sendButton").addEventListener("click", function (event) {
// 	var user = document.getElementById("userInput").value;
// 	var message = document.getElementById("messageInput").value;
// 	var groupName = document.getElementById("groupInput").value;
// 	connection.invoke("SendMessage", user, message, groupName).catch(function (err) {
// 		return console.error(err.toString());
// 	});
// 	event.preventDefault();
// });

