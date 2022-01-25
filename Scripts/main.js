var chat;
var initialize = () => {
	var match = document.cookie.match(new RegExp("(^| )" + "story-estimation" + "=([^;]+)"));
	if (match) $("#judge").val(match[2]);

	chat = $.connection.signalRHub;
	chat.client.addNewMessageToPage = function (name, message) {
		renderVotes(message);
	};

	$.connection.hub.start().done(function () {
		loadCurrentStory();
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
		chat.server.send(name, "NewPoint");
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
		$("#myPoint").text(data.Point);
	} );
}

var startNewVote = () => {
	var name = $( "#judge" ).val();

	$.get( `/home/StartNewVote`, () => {
		chat.server.send(name, "StartNewVote");
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

var getScoreExplanation = (score) => {
	switch (score)
	{
	case 1:
	case 2:
		return `${score} (Very Low)`;

	case 3:
	case 4:
		return `${score} (Low)`;

	case 5:
	case 6:
		return `${score} (Medium)`;

	case 7:
	case 8:
		return `${score} (High)`;

	case 9:
	case 10:
		return `${score} (Very High)`;

	default:
		return `${score} (Very Low)`;
	}
}

var renderVotes = (story) => {
	if (story.PointsWithAverage.Count <= 1) return;

	var html = "";
	story.PointsWithAverage.forEach(pair => 
        html += 
		`<div class="row">
            <div class="col-md-2">
                <h3 class="${pair.Value.IsEmpty ? "bg-danger" : "bg-success"}">${pair.Key}</h3>
            </div>

            <div class="col-md-1">
                <div class="slidecontainer">
                    <label>Point</label>
                    <h2 class="score">${story.Showing ? pair.Value.Point.toString() : "?"}</h2>
                </div>
            </div>

            <div class="col-md-3">
                <div class="slidecontainer">
                    <label>Complexity ${story.Showing ? getScoreExplanation(pair.Value.Complexity) : "?"}</label>
                    <input type="range" class="slider" min="0" max="10" value="${story.Showing ? pair.Value.Complexity.toString() : "0"}" disabled>
                </div>
            </div>

            <div class="col-md-3">
                <div class="slidecontainer">
                    <label>Effort ${story.Showing ? getScoreExplanation(pair.Value.Effort) : "?"}</label>
                    <input type="range" class="slider" min="0" max="10" value="${story.Showing ? pair.Value.Effort.toString() : "0"}" disabled>
                </div>
            </div>
            <div class="col-md-3">
                <div class="slidecontainer">
                    <label>Uncertainty ${story.Showing ? getScoreExplanation(pair.Value.Uncertainty) : "?"}</label>
                    <input type="range" class="slider" min="0" max="10" value="${story.Showing ? pair.Value.Uncertainty.toString() : "0"}" disabled>
                </div>
            </div>
        </div>`);
		$('#votes').html(html);

		$('#showHideButton').html(
			`<span class="btn-label">
			<i class="glyphicon ${story.Showing ? "glyphicon glyphicon-eye-close" : "glyphicon-eye-open"}"></i>
			</span>${story.Showing ? "Hide Votes" : "Show Votes"}`);
}

var loadCurrentStory = () => {
	$.get( `/home/CurrentStory`, (story) => {
		renderVotes(story);
	});
}