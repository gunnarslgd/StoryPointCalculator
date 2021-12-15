var chat;
var initialize = () => {
	var match = document.cookie.match(new RegExp("(^| )" + "story-estimation" + "=([^;]+)"));
	if (match) $("#judge").val(match[2]);

	chat = $.connection.signalRHub;
	chat.client.addNewMessageToPage = function (name, message) {
		setTimeout(() => {
				location.href = location.href;
			},
			Math.floor(Math.random() * 1000));
	};

	$.connection.hub.start().done(function () {
	});
}

var showError = (message) => {
	Toastify({
		text: message,
		duration: 3000,
		position: "left",
		style: {
			background: "linear-gradient(to right, #00b09b, #FF8333)",
		}
	}).showToast();
};

var joinEstimation = () => {
	var name = $( "#judge" ).val();

	if (name === "") {
		showError("Please enter your name.");
		return;
	}

	$.get( `/home/JoinEstimation?name=${name}`, () => {
		chat.server.send(name, "JoinEstimation");
	});
}

var submit = () => {
	var name = $( "#judge" ).val();
	var complexity = $( "#complexity" ).val();
	var effort = $( "#effort" ).val();
	var uncertainty = $( "#uncertainty" ).val();

	if (name === "") {
		showError("Please enter your name.");
		return;
	}

	$.post( `/home/NewPoint?name=${name}`, { complexity, effort, uncertainty }, (story) => {
		if (story.Showing) chat.server.send(name, "NewPoint");
	} );
}
 
var submitSilently = () => {
	var name = $( "#judge" ).val();
	var complexity = $( "#complexity" ).val();
	var effort = $( "#effort" ).val();
	var uncertainty = $( "#uncertainty" ).val();

	if (name === "") {
		showError("Please enter your name.");
		return;
	}

	$.post( `/home/NewPointSilently?name=${name}`, { complexity, effort, uncertainty }, (data) => {
		$("#complexity").prev().text(`Complexity ${data.Complexity}`);
		$("#effort").prev().text(`Effort ${data.Effort}`);
		$("#uncertainty").prev().text(`Uncertainty ${data.Uncertainty}`);
		console.log(data);
	} );
}

var clearAllEstimations = () => {
	var name = $( "#judge" ).val();

	$.get( `/home/ClearAllEstimations`, () => {
		chat.server.send(name, "ClearAllEstimations");
	});
}

var showEstimations = () => {
	var name = $( "#judge" ).val();

	$.get( `/home/ShowEstimations`, () => {
		chat.server.send(name, "ShowEstimations");
	});
}

var showingPanel = true;
var showPanel = () => {
	if (showingPanel) {
		$("#pointing-panel").hide();
		$("#collapse-button").attr("class","glyphicon glyphicon-menu-up");
	} else {
		$("#pointing-panel").show();
		$("#collapse-button").attr("class","glyphicon glyphicon-menu-down");
	}
	showingPanel = !showingPanel;
}

var removeAllJudges = () => {
	var name = $( "#judge" ).val();

	$.get( `/home/RemoveAllJudges`, () => {
		chat.server.send(name, "RemoveAllJudges");
	});
}