part of 'home_bloc.dart';

abstract class HomeEvent {}

class AddDescription extends HomeEvent {
  final String description;
  AddDescription(this.description);
}

class RemoveDescription extends HomeEvent {
  final String descriptionId;
  RemoveDescription(this.descriptionId);
}

class UpdateStatus extends HomeEvent {
  final String descriptionId;
  final bool descriptionStatus;
  UpdateStatus(this.descriptionId, this.descriptionStatus);
}

class getAllDescription extends HomeEvent {
  getAllDescription();
}
