import 'package:flutter/material.dart';
import 'package:flutter_app/home/home.dart';
import 'package:flutter_app/login/bloc/login_bloc.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

class Login extends StatelessWidget {
  Login({super.key});
  final usernameController = TextEditingController();
  final passwordController = TextEditingController();

  void _onEvent(BuildContext context, String username, String password) {
    final blos = context.read<LoginBloc>();
    blos.add(LoginSubmitted(username, password));
    // if (blos.state is LoginSuccess) {
    //   Navigator.push(
    //     context,
    //     MaterialPageRoute(builder: (context) => Home(username: username)),
    //   );
    // }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Login')),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: <Widget>[
            Form(
              child: Column(
                children: <Widget>[
                  TextFormField(
                    decoration: const InputDecoration(
                      labelText: 'Username',
                      prefixIcon: Icon(Icons.person),
                    ),
                    controller: usernameController,
                  ),
                  const SizedBox(height: 20),
                  TextFormField(
                    decoration: const InputDecoration(
                      labelText: 'Password',
                      prefixIcon: Icon(Icons.lock),
                    ),
                    controller: passwordController,
                    obscureText: true,
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),
            ElevatedButton(
              onPressed:
                  () => _onEvent(
                    context,
                    usernameController.text,
                    passwordController.text,
                  ),
              child: const Text('Login'),
            ),
          ],
        ),
      ),
    );
  }
}
