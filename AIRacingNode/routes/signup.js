var express = require('express');
var router = express.Router();

router.get('/', function(req, res) {
    res.render('signup', {  title: 'Register' , message: req.flash('signupMessage') })
});

// process the signup form
router.post('/', function(req, res) {
    return req.passport.authenticate('local-signup', {
        successRedirect : '/profile', // redirect to the secure profile section
        failureRedirect : '/signup', // redirect back to the signup page if there is an error
        failureFlash : true // allow flash messages
    });
});

module.exports = router;
