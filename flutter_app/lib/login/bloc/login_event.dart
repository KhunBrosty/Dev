part of 'login_bloc.dart';

abstract class LoginEvent {}

// Events
class LoginSubmitted extends LoginEvent {
  final String username;
  final String password;
  LoginSubmitted(this.username, this.password);
}

class GetUser extends LoginEvent {
  GetUser();
}

class LoginLogout extends LoginEvent {
  LoginLogout();
}
