part of 'home_bloc.dart';

abstract class HomeState {}

class HomeInitial extends HomeState {}

class HomeLoaded extends HomeState {
  // ignore: prefer_typing_uninitialized_variables
  final description;
  HomeLoaded(this.description);
}
