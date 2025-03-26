part of 'login_bloc.dart';

abstract class LoginState {}

// initial state is the starting point
class LoginInitial extends LoginState {}

class LoginSuccess extends LoginState {
  final String message;
  LoginSuccess(this.message);
}