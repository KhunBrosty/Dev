import 'package:flutter_app/login/repository/login_repository.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

part 'login_event.dart';
part 'login_state.dart';

// BLoC
class LoginBloc extends Bloc<LoginEvent, LoginState> {
  final Loginrepository _loginrepository;

  LoginBloc(this._loginrepository) : super(LoginInitial()) {
    on<LoginSubmitted>((event, emit) async {
      try {
        await _loginrepository.login(event.username, event.password);
        emit(LoginSuccess("Loggin Success"));
      } catch (e) {
        print(e.toString());
      }
    });
  }
}