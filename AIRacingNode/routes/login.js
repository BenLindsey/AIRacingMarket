var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    res.render('login', { message: req.flash('loginMessage') });
});

router.post('/', function(req, res) {
  return req.passport.authenticate('local-login', {
      successRedirect: '/leaderboard',
      failureRedirect: '/login', // redirect back to the signup page if there is an error
      failureFlash: true // allow flash messages
    })
});

module.exports = router;
