theforum.ForumService = function () {
    let self = this;

    self.registerAccount = function (username, emailAddress, password, passwordConfirm) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.registerAccount,
            data: ko.toJSON({ Username: username, EmailAddress: emailAddress, Password: password, PasswordConfirm: passwordConfirm }),
            contentType: 'application/json'
        });
    };

    self.requestPasswordReset = function (emailAddress) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.requestPasswordReset,
            data: ko.toJSON({ EmailAddress: emailAddress }),
            contentType: 'application/json'
        });
    };

    self.verifyPasswordResetEmail = function (emailAddress) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.verifyPasswordResetEmail,
            data: ko.toJSON({ EmailAddress: emailAddress }),
            contentType: 'application/json'
        });
    };

    self.updatePassword = function (emailAddress, authenticationCode, password, passwordConfirm) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.updatePassword,
            data: ko.toJSON({
                EmailAddress: emailAddress,
                AuthenticationCode: authenticationCode,
                Password: password,
                PasswordConfirm: passwordConfirm
            }),
            contentType: 'application/json'
        });
    };

    self.login = function (username, password) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.login,
            data: ko.toJSON({ Username: username, Password: password }),
            contentType: 'application/json'
        });
    };

    self.logout = function () {
        return $.post(theforum.constants.api.logout);
    };

    self.getPosts = function () {
        return $.get(theforum.constants.api.getPosts);
    };

    self.getThreads = function () {
        return $.get(theforum.constants.api.getThreads);
    };

    self.getThreadsSorted = function (sortOrder) {
        return $.get(theforum.constants.api.getThreadsSorted.format(sortOrder));
    };

    self.getThreadsWithPageNumber = function (pageNumber) {
        return $.get(theforum.constants.api.getThreadsWithPageNumber.format(pageNumber));
    };

    self.getThreadsSortedWithPageNumber = function (sortOrder, pageNumber) {
        return $.get(theforum.constants.api.getThreadsSortedWithPageNumber.format(sortOrder, pageNumber));
    };

    self.getThreadReplies = function (threadId) {
        return $.get(theforum.constants.api.getThreadReplies.format(threadId));
    };

    self.createThread = function (title, message) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.createThread,
            data: ko.toJSON({ Title: title, Message: message }),
            contentType: 'application/json'
        });
    };

    self.createReplyToThread = function (threadId, title, message) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.createReplyToThread.format(threadId),
            data: ko.toJSON({ Title: title, Message: message }),
            contentType: 'application/json'
        });
    };

    self.createReplyToPost = function (threadId, postId, title, message) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.createReplyToPost.format(threadId, postId),
            data: ko.toJSON({ Title: title, Message: message }),
            contentType: 'application/json'
        });
    };

    self.createPost = function (post) {
        return $.ajax({
            type: "POST",
            url: theforum.constants.api.createPost,
            data: ko.toJSON(post),
            contentType: 'application/json'
        });
    };

    self.resetUrl = function () {
        window.history.pushState(null, null, theforum.constants.urls.base);
    };

    self.addUrlToVisited = function (url) {
        history.replaceState(null, null, url);
        history.replaceState(null, null, theforum.constants.urls.base)
    };
};