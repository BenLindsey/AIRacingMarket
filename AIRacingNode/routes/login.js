var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    res.render('login', { message: req.flash('loginMessage') });
});

router.post('/', function(req, res, next) {
  return req.passport.authenticate('local-login', function(err, user, info){

      if (err)   { return next(err); }
      if (!user) { return res.redirect('/login'); }
      
      req.login(user, function(err) {
        if (err) { return next(err); }
        return res.redirect(next.successRedirect);
      });
      //successRedirect: '/script',
      //failureRedirect: '/login', // redirect back to the signin page if there is an error
      //failureFlash: true // allow flash messages
  })(req, res, next);
});

module.exports = router;
