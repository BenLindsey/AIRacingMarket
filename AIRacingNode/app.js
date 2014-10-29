var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var passport = require('passport');
var flash = require('connect-flash');
var session = require('express-session');
var mongoose = require('mongoose');

var configDB = require('./config/database.js');

mongoose.connect(configDB.url); // connect to our database

require('./config/passport')(passport); // pass passport for configuration

var mongo = require('mongodb');
var monk = require('monk');
var db = monk(configDB.url);

var routes = require('./routes/index');
var script = require('./routes/script');
var tournament = require('./routes/tournament');
var time = require('./routes/time');
var leaderboard = require('./routes/leaderboard');
var login = require('./routes/login');
var logout = require('./routes/logout');
var signup = require('./routes/signup');
var profile = require('./routes/profile');

var app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

// uncomment after placing your favicon in /public
//app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

app.use(session({ secret: 'cakeissogoodomgguysseriously'}));
app.use(passport.initialize());
app.use(passport.session());
app.use(flash());

// pass passport/mongo to the router
app.use(function(req, res, next) {
    req.db = db;
    req.passport = passport;
    res.locals.user = req.user || null;
    next();
});

app.use('/', routes);
app.use('/script', script);
app.use('/time', time);
app.use('/leaderboard', leaderboard);
app.use('/login', login);
app.use('/logout', logout);
app.use('/signup', signup);
app.use('/profile', profile);
app.use('/tournament', tournament);

// catch 404 and forward to error handler
app.use(function(req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function(err, req, res, next) {
        res.status(err.status || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});

module.exports = app;
