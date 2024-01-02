theforum.Reply = function (data, treeRow, isLastReplyInTreeColumn) {

    let buildTreeRow = function (parentTreeRow, isLastReply) {
        let treeRow;
        if (parentTreeRow && parentTreeRow.length > 0) {
            treeRow = parentTreeRow.slice();
        } else {
            treeRow = parentTreeRow || [];
        }
        treeRow.push(isLastReply);
        return treeRow;
    };

    let self = this;

    self.id = ko.observable(data ? data.Id : null);
    self.threadId = ko.observable(data ? data.ThreadId : null);
    self.title = ko.observable(data ? data.Title : null);
    self.message = ko.observable(data ? data.Message : null);
    self.postedByUsername = ko.observable(data ? data.PostedByUsername : null);
    self.postedByEmailAddress = ko.observable(data ? data.PostedByEmailAddress : null);
    self.createdDateTimeUtc = ko.observable(data ? moment.utc(data.CreatedDateTimeUtc) : null);
    self.treeRow = ko.observableArray(buildTreeRow(treeRow, isLastReplyInTreeColumn));
    self.replies = ko.observable(data ?
        ko.utils.arrayMap(data.Replies, function (replyData, index) {
            let isLastReply = data.Replies.length === index + 1;
            return new theforum.Reply(replyData, self.treeRow(), isLastReply);
        }) : null);
    self.isSelected = ko.observable(false);

    self.url = ko.computed(function () {
        return theforum.constants.urls.reply.format(self.threadId(), self.id());
    });

    self.createdDateTime = ko.computed(function () {
        if (self.createdDateTimeUtc()) {
            return self.createdDateTimeUtc().local();
        }
    });

    self.createdDayTimeString = ko.computed(function () {
        return self.createdDateTime().calendar() + " " + self.createdDateTime().format("HH:mm");
    });

    self.createdTimeString = ko.computed(function () {
        return self.createdDateTime().format("HH:mm");
    });

    self.createdDateString = ko.computed(function () {
        return self.createdDateTime().format("DD.MM.YY");
    });

    self.mailtoUrl = ko.computed(function () {
        return theforum.constants.mailtoTemplate.format(self.postedByEmailAddress());
    });
}