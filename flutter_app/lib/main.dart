import 'package:flutter/material.dart';
import 'package:flutter_app/login/bloc/login_bloc.dart';
import 'package:flutter_app/login/login.dart';
import 'package:flutter_app/login/repository/login_repository.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: BlocProvider(
        create: (context) => LoginBloc(Loginrepository()),
        child: Login(),
      ),
    );
  }
}