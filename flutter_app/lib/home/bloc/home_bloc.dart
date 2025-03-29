import 'package:flutter_app/home/repository/home_repository.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

part 'home_event.dart';
part 'home_state.dart';

class HomeBloc extends Bloc<HomeEvent, HomeState> {
  final HomeRepository _homeRepository;

  HomeBloc(this._homeRepository) : super(HomeInitial()) {
    on<AddDescription>((event, emit) async {
      try {
        await _homeRepository.addDescription(event.description);
        emit(HomeLoaded("Completed"));
      } catch (e) {
        print(e.toString());
      }
    });

    // on<RemoveDescription>((event, emit) async {
    //   try {
    //     //await _homeRepository.removeDescription(event.descriptionId);
    //     emit(HomeLoaded(event.descriptionId));
    //   } catch (e) {
    //     print(e.toString());
    //   }
    // });

    // on<UpdateStatus>((event, emit) async {
    //   try {
    //     //await _homeRepository.updateStatus(event.descriptionId, event.descriptionStatus);
    //     emit(HomeLoaded(event.descriptionId));
    //   } catch (e) {
    //     print(e.toString());
    //   }
    // });
  }
}
