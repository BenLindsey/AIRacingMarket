var Browser = require("zombie");
var assert = require("assert");
var app = require("../app");

describe('login page', function() {

  before(function() {
    server = app.listen(2999);
  });

  beforeEach(function() {
    Browser.localhost('146.169.47.15', 2999);
    browser = Browser.create();
  });

  it('should visit the login page', function(done) {
    this.timeout(4000);
    browser.visit('/login', function() {
      assert.equal(browser.location.pathname, "/login");
      done();
    })
  });

  it('should redirect to home on login', function(done) {
    this.timeout(4000);
    browser.visit('/login')
      .then(function() {
        browser.fill("email", "a@b.com");
        browser.fill("password", "c");
        return browser.pressButton("Submit");
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/");
        browser.assert.text("title", "AI Racing Market");
        done();
      })
  });

  it('should stay on login if email address is invalid', function(done) {
    this.timeout(4000);
    browser.visit('/login')
      .then(function() {
        browser.fill("email", "bad@login.com");
        browser.fill("password", "badpassword");
        return browser.pressButton("Submit");
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/login");
        done();
      })
  });

  it('should stay on login if email is correct but password is invalid', function(done) {
    this.timeout(4000);
    browser.visit('/login')
      .then(function() {
        browser.fill("email", "a@b.com");
        browser.fill("password", "badpassword");
        return browser.pressButton("Submit");
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/login");
        done();
      })
  });

  it('should redirect you to the registration page if the user hasnt already', function(done) {
    this.timeout(4000);
    browser.visit('/login')
      .then(function() {
        return browser.visit(browser.link("Click here to register."));
      })
      .done(function() {
        browser.assert.success();
        assert.equal(browser.location.pathname, "/signup");
        done();
      })
  });

  after(function() {
    server.close();
  });

});
