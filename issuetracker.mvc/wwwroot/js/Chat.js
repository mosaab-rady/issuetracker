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
					${commentText.split('\n').map(elm => {
		return `<p class="text-dark mb-0">
		${elm}
		</p>`
	}).join('')}
					</div>
				</div>`

	document.getElementById("CommentsDiv").insertAdjacentHTML("beforeend", newComment);
	document.getElementById("CommentsDiv").scrollTop = document.getElementById("CommentsDiv").scrollHeight;
	const sound = document.getElementById("audio");
	sound.play();

});

connection.start().then(function () {
	var groupName = document.getElementById("groupNameInput").value;
	connection.invoke("JoinGroup", groupName);
}).catch(function (err) {
	return console.error(err.toString());
});



let CommentForm = document.getElementById("createComment")

if (CommentForm) {
	CommentForm.addEventListener("submit", (e) => {
		e.preventDefault();
		submitComment(e);
	});
}

function submitComment(e) {
	const comment = e.target.Comment.value;
	const issueId = e.target.IssueId.value;

	const data = { Comment: comment, IssueId: issueId };

	fetch(`/issues/CreateComment?issueId=${issueId}`, {
		method: 'POST',
		body: JSON.stringify(data),
		headers: {
			'Content-Type': 'application/json',
		},
	}).then(res => {
		console.log(res);
		if (res.status == 200) {
			document.getElementById("CommentInp").value = '';
		}
	})
}


