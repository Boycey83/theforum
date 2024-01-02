theforum.constants = {
    api: {
        registerAccount: "/api/useraccount/register",
        requestPasswordReset: "/api/useraccount/requestpasswordreset",
        verifyPasswordResetEmail: "/api/useraccount/verifypasswordresetemail",
        updatePassword: "/api/useraccount/updatepassword",
        login: "/api/useraccount/login",
        logout: "/api/useraccount/logout",
        getThreads: "/api/forum/threads",
        getThreadsWithPageNumber: "/api/forum/threads/page/{0}",
        getThreadsSorted: "/api/forum/threads/sort/{0}",
        getThreadsSortedWithPageNumber: "/api/forum/threads/sort/{0}/page/{1}",
        createThread: "/api/forum/thread",
        createReplyToThread: "/api/forum/thread/{0}/reply",
        createReplyToPost: "/api/forum/thread/{0}/post/{1}/reply",
        getThreadReplies: "/api/forum/thread/{0}/replies"
    },
    urls: {
        base: "/",
        thread: "/thread/{0}?p={1}",
        reply: "/thread/{0}/reply/{1}"
    },
    pageSize: 25,
    mailtoTemplate: "mailto:{0}",
    sortOrder: {
        date: "date",
        dateDesc: "datedesc",
        postedBy : "postedby",
        postedByDesc: "postedbydesc",
        subject: "subject",
        subjectDesc: "subjectdesc",
        size: "size",
        sizeDesc: "sizedesc"
    },
    defaultSortOrder: "datedesc"
};
