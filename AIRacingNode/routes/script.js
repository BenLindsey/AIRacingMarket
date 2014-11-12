var express = require('express');
var router = express.Router();

router.get('/edit/:name', isLoggedIn, function(req, res) {
    var db = req.db;

    var collection = db.get('scriptcollection');

    collection.findOne({scriptName:req.params.name, email : req.user.local.email},  function(e, doc) {
        res.render('edit', {
          script : doc.script,
          scriptName : req.params.name
        });
    });
});

router.get('/', function(req, res) {
    res.render('script', { script : 
        "// The vehicle can be controlled by calling functions on the api object\n" +
        "// e.g. api.SetThrottle()\n" +
        "\n" +
        "// Global state can be stored in the global data object\n" +
        "// e.g. data[\"count\"] = 10;\n" +
        "//      data[\"count\"] = data[\"count\"] - 1;\n" +
        "\n" +
        "// Called once when the script is loaded\n" +
        "var Init = function() {\n" +
        "\n" +
        "};\n" +
        "\n" +
        "// Called repeatedly as the game is running\n" +
        "var PhysicsUpdate = function() {\n" +
        "\n" +
        "};",
        notLoggedIn : !req.isAuthenticated()
    });
});

/* GET the contents of a script by name */
router.get('/:name', function(req, res) {
    var db = req.db;

    var collection = db.get('scriptcollection');

    collection.findOne({scriptName:req.params.name},  function(e, doc) {
        res.send(doc.script, 200);
    });
});

/* POST to script service */
router.post('/', function(req, res) {
    var collection = req.db.get('scriptcollection');

    //TODO CHECK IF USER LOGGED IN
    console.log("user :" );
    console.log(req.user);

    collection.insert({
        "email"      : req.user.local.email,
        "scriptName" : req.body.scriptname,
        "script"     : req.body.script
    }, function (err, doc) {
        if (err) {
            res.send("There was a problem adding the information to the database.");
        }
        else {
            var url = "/profile";

            console.log("Redirecting user to: " + url);
            // If it worked, set the header so the address bar doesn't still say /script
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

router.post('/edit/:name', isLoggedIn, function(req, res) {
    var collection = req.db.get('scriptcollection');

    console.log("user :" );
    console.log(req.user);

    collection.update({
        "scriptName" : req.body.scriptname
    }, {
        "email"      : req.user.local.email,
        "scriptName" : req.body.scriptname,
        "script"     : req.body.script
    }, function (err, doc) {
        if (err) {
            res.send("There was a problem adding the information to the database.");
        }
        else {
            var url = "/profile";

            console.log("Redirecting user to: " + url);
            // If it worked, set the header so the address bar doesn't still say /script
            res.location(url);
            // And forward to success page
            res.redirect(url);
        }
    });
});

function isLoggedIn(req, res, next) {

    req.session.redirect = '/script';

    // if user is authenticated in the session, pass to GET/POST handlers
    if (req.isAuthenticated()) {
        return next();
    }

    // if they aren't redirect them to the login page
    res.redirect('/login');
}

module.exports = router;
