var express = require('express');
var router = express.Router();

/* GET New User page. */
router.get('/', function(req, res) {
    res.render('login', { message: req.flash('loginMessage') });
});

router.post('/', function(req, res, next) {
  return req.passport.authenticate('local-login', function(err, user, info){

      if (err)   { 
        res.locals.user = null;
        return next(err); 
      }

      if (!user) { 
        res.locals.user = null;
        return res.redirect('/login'); 
      }
      
      req.login(user, function(err) {
        if (err) { 
          return next(err); 
        }

        res.locals.user = req.user || null;
        if (!req.session.redirect) {
          return res.redirect('/');
        }
        return res.redirect(req.session.redirect);
      });
  })(req, res, next);
});

module.exports = router;
