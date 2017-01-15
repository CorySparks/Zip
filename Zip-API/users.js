var express = require('express');
var router = express.Router();
var ObjectId = require('mongodb').ObjectID;
//var Sha256 = require('../libs/Crypto.js');

/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});


var exists = function(obj){
  return (typeof(obj) != 'undefined') ? true : false;
}

var phpTime = function(){
  return Math.floor(new Date().getTime() / 1000);
}

/*{
  tokens.get("i4uf29j0d1j239dk230j240d", (lvl) => {
    tokens.session.create(lvl, docs._id, (tkn) => {

    })
  })
}
*/
var tokens = { //Implement Callbacks
  db: undefined,
  refresher: {
    create: function(userID, cb){ //refresher token
      var createToken = function(_userID, _cb){
        var token = {
          "token": ObjectId(),
          "type": 3,
          "expire": 0,
          "created": phpTime(),
          "creator": _userID
        }
        tokens.db.get('tokens').insert(token, function(e, docs){
          if(!e){
            delete docs._id;
            delete docs.created;
            delete docs.type;
            delete docs.creator;
            _cb(docs);
          }else{
            _cb({errorCode: 1, error: "check console."}); console.log(err);
          }
        });
      }
      if(!exists(tokens.db)) cb({errorCode: 1, error: "No DB Ref"}); else createToken(userID, cb);
      //(exists(tokens.db)) ? createToken(cb) : cb({errorCode: 1, error: "No DB Ref"});
    }
  },
  session: {
    create: function(tknLvl, userID, cb){ //refresher token
      //console.log("refToken: " + refToken);
      var createToken = function(_token, _userID, _cb){
        //console.log("_token: " + _token);
        if(tknLvl == 3){
          var token = {
            "token": ObjectId(),
            "type": 2,
            "expire": phpTime() + (24*60*60), //1 DAY
            "created": phpTime(),
            "creator": _userID
          }
          tokens.db.get('tokens').insert(token, function(e, docs){
            if(!e){
              delete doc._id;
              _cb(docs);
            }else{
              _cb({errorCode: 1, error: "check console."}); console.log(err);
            }
          });
        }else{
          _cb({errorCode: 1, error: "Invalid Token"});
        }
      }
      if (!exists(tokens.db)) cb({errorCode: 1, error: "No DB Ref"}); else createToken(tknLvl, userID, cb);
      //(exists(tokens.db)) ? createToken(refToken, cb) : cb({errorCode: 1, error: "No DB Ref"});
    }
  },
  get: {
    type: function(token, cb){ // cb token type (int)
      var getType = function(_token, _cb){
        var tkn = ObjectId(_token);
        tokens.db.get('tokens').find({ $query: {token: tkn}}, { fields : { _id:0, created:0}, limit : 1 }, function(e,docs){
          _cb(docs[0].type);
        });
      }
      if (!exists(tokens.db)) cb({errorCode: 1, error: "No DB Ref"}); else getType(token, cb);
      //(exists(tokens.db)) ? getType(token, cb) : cb({errorCode: 1, error: "No DB Ref"});
    }
  },
  verify: function(token, lvl, cb){ // cb true/false if token type is lvl
    var checkExpired = function(_docs, _cb){
      if(_docs[0].expire != 0 && _docs[0].expire < phpTime()){
        tokens.destroy(_docs[0].token, function(bool){
          _cb(bool);
        });
      }else{
        _cb(false);
      }
    }
    var verification = function(_token, _lvl, _cb){
      //console.log("Token:" + _token);
      var tkn = ObjectId(_token);
      tokens.db.get('tokens').find({ $query: {token: tkn}}, { fields : { _id:0, created:0}, limit : 1 } ,function(e,docs){
        if(docs){
          checkExpired(docs, function(bool){
            if (bool) _cb(false); else _cb(docs[0].type == _lvl);
          });
        }else{
          _cb(false);
        }
      });
    }
    if (!exists(tokens.db)) cb({errorCode: 1, error: "No DB Ref"}); else verification(token, lvl, cb);
    //(exists(tokens.db)) ? verification(token, lvl, cb) : cb({errorCode: 1, error: "No DB Ref"});
  },
  destroy: function(token, cb){
    var delToken = function(_token, _cb){
      try{
        var tkn = ObjectId(_token); // what if he's a dumb ass tryna hack you
        tokens.db.get('tokens').remove({token: tkn}, 1);
        _cb(true, false);
      }catch(e){
        console.log("nigga tryna fuck with the token destroy lib lol");
        _cb(false);
      }
    }
    if (!exists(tokens.db)) cb({errorCode: 1, error: "No DB Ref"}); else delToken(token, cb);
  }
};
//TOKEN LIBRARY END

router.post('/TokenCheck', function(req, res){
  console.log('Made it to the /TokenCheck');
  CheckToken(req.db, req.body.Token, req.body.Expire, function(pass){
    console.log('Made it back from the CheckToken func');
    if(pass){
      res.send('1');
    }else{
      console.log('Sent 0');
      res.send('0');
    }
  });
});

router.post('/token/create', function(req, res){
  var err = false;
  var handleError = function(e){
    if(!err){
      err = true;
      var eDoc = {};
      eDoc.errorCode = 1;
      eDoc.error = e;
      res.send(eDoc);
    }
  }
  var userID = req.body.userID || handleError("no userID");
  if(!err){
    tokens.db = req.db;
    tokens.refresher.create(userID, function(token){
      res.send('1');
    });
  }
});

router.post('/createUser', function(req, res){
	if(req.body.User, req.body.Pass, req.body.Email){
		if((2 <= req.body.User.length <= 16)){
			if(8 <= req.body.Pass.length <= 64){
				if(req.body.Pass === req.body.ConfirmPass){
					inUse(req.db, req.body.User, function(val){
						if(!val){
							var doc = {
								username: { user: req.body.User, search: req.body.User.toLowerCase() },
								pass: req.body.Pass,
								email: req.body.Email
							}
							var db = req.db.get('Users');
						    db.insert(doc, function(e, docs){
						    	if(!e){
						    		res.send('1');
						    		//console.log(db);
						    	}else{
						    		res.send('0');
						    		console.log(e);
						    	}
						    });
						}else{
							res.send('User Taken')
						}
					});

				    //0 = false
				    //1 = Passed
				    //2 = It seems as if your passwords do not match. Please try again.
				    //3 = Your password must exceed the minimum of 8 characters but less than 24 characters.
				    //4 = Your username must exceed the minimum of two characters but less than 16 characters.
				    //5 = You have to complete all fields.
				}else{
					res.send('2')
				}
			}else{
				res.send('3')
			}
		}else{
			res.send('4')
		}
	}else{
		res.send('5')
	}
});

router.post('/loginUser', function(req, res){
	CheckingLogin(req.db, req.body.username, req.body.password, function(bool, docs){
		if(bool && exists(docs)){
        var err = false;
        var handleError = function(e){
        if(err){
          err = true;
          var eDoc = {};
          eDoc.errorCode = 1;
          eDoc.error = e;
          console.log(eDoc);
        }
      }
      var userID = docs[0]._id;
      if(userID != null){
        if(!err){
          tokens.db = req.db;
          tokens.refresher.create(userID, function(token){
            var tokenDoc = {
              pass: '1',
              token: token.token,
              expire: token.expire
            }
            var jDoc = JSON.stringify(tokenDoc);
            res.send(jDoc);
          });
        }else{
          console.log('Token err = true');
        }
      }else{
        console.log('Creator token = null');
        res.send('0');
      }
		}else{
			res.send('0');
		}
	});
});

router.get('/createRandAmount', function(req, res){
  var arrayGenerator = function(cb){
    var generate = function(cb){
      if(Math.floor((Math.random() * 100) + 1) >= 3){
    		switch(Math.floor((Math.random() * 10) + 1)){
      		case 1:
      			cb('Zip');
          	break;
      		case 2:
          	cb('10\xA2');
          	break;
          case 3:
        		cb('Zip');
        		break;
        	case 4:
        		cb('2x');
        		break;
        	case 5:
      		  cb('Zip');
          	break;
        	case 6:
        		cb('Zip');
        		break;
        	case 7:
          	cb('Zip');
        		break;
        	case 8:
        		cb('3x');
        		break;
        	case 9:
    		    cb('Zip');
          	break;
        	case 10:
        		cb('5\xA2');
        		break;
    		  default:
      		  cb('0');
            break;
    		}
      }else{
        if(Math.floor((Math.random() * 100) + 1) >= 2){
          switch(Math.floor((Math.random() * 10) + 1)){
        		case 1:
        			cb('20\xA2');
            	break;
        		case 2:
            	cb('15\xA2');
            	break;
            case 3:
          		cb('Zip');
          		break;
          	case 4:
          		cb('2x');
          		break;
          	case 5:
        		  cb('20\xA2');
            	break;
          	case 6:
          		cb('Zip');
          		break;
          	case 7:
            	cb('Zip');
          		break;
          	case 8:
          		cb('3x');
          		break;
          	case 9:
      		    cb('15\xA2');
            	break;
          	case 10:
          		cb('15\xA2');
          		break;
      		  default:
        		  cb('0');
              break;
      		}
        }else{
          switch(Math.floor((Math.random() * 10) + 1)){
        		case 1:
        			cb('50\xA2');
            	break;
        		case 2:
            	cb('75\xA2');
            	break;
            case 3:
          		cb('10\xA2');
          		break;
          	case 4:
          		cb('$1.00');
          		break;
          	case 5:
        		  cb('15\xA2');
            	break;
          	case 6:
          		cb('30\xA2');
          		break;
          	case 7:
            	cb('40\xA2');
          		break;
          	case 8:
          		cb('3x');
          		break;
          	case 9:
      		    cb('50\xA2');
            	break;
          	case 10:
          		cb('10\xA2');
          		break;
      		  default:
        		  cb('0');
              break;
      		}
        }
      }
    }
    var moves = [];
    for(var i = 0; i < 16; i++){
      generate(function(move){
        moves.push(move);
      });
    }

    cb(moves);
  }
  arrayGenerator(function(newArray){
    var LandIndex = Math.floor(Math.random() * 16);
    var LandAmount = newArray[LandIndex];
    var doc = {
      moves: newArray,
      index: LandIndex,
      won: LandAmount
    }
    var jDoc = JSON.stringify(doc);
    console.log(jDoc);
    res.send(jDoc);
  });
});

function CheckToken(db, token, expire, cb){
  console.log('Made it to the CheckToken func');
  var _db = db.get('tokens');
  _db.find({'token': token}), function(e, doc){
    console.log('Made it in the db.find');
    if(!e){
      if(doc.length > 0){
        if(expire < phpTime()){
          cb(true);
        }else{
          console.log('Time is messed up');
          cb(false);
        }
      }else{
        console.log('Docs Is Messed Up');
      }
    }else{
      console.log(e);
    }
  }
}

function CheckingLogin(db, username, password, cb){
	var _db = db.get('Users');
	_db.find(({'username.search': username.toLowerCase(), 'pass': password }), function(e, docs){
		if(!e){
			if(docs.length > 0){
				cb(true, docs);
			}else{
				cb(false);
			}
		}else{
			console.log(e);
			cb(false);
		}
	});
}

function inUse(db, username, cb){
	var _db = db.get('Users');
	_db.find({'username.search': username.toLowerCase() }, function(e, docs){
    if(!e){
      if(docs.length > 0){
    		cb(true);
    	}else{
        	cb(false);
    	}
    }else{
      cb(true);
      console.log("Error in the inUse function; is mongodb running?");
      console.log(e);
    }
  });
}

module.exports = router;

/*

*/
