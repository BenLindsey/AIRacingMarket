var express = require('express');
var router = express.Router();

/* GET user profile. */
router.get('/', isLoggedIn, function(req, res) {
    res.render('webbuild', { } );
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/webbuild';

    if (req.isAuthenticated())
        return next();

    res.redirect('/login');
}

module.exports = router;
