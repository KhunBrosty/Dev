import 'package:flutter/material.dart';
import 'package:flutter_app/home/bloc/home_bloc.dart';
import 'package:flutter_app/home/repository/home_repository.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

class Home extends StatelessWidget {
  Home({super.key, required this.username});
  final String username;
  final descriptionController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => HomeBloc(HomeRepository()),
      child: BlocListener<HomeBloc, HomeState>(
        listener: (context, state) {
          if (state is HomeLoaded) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Description added successfully')),
            );
          }
        },
        child: Scaffold(
          appBar: AppBar(title: Text('Home')),
          body: Center(
            child: Container(
              padding: const EdgeInsets.all(20),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  Text('Welcome $username'),
                  TextFormField(
                    decoration: InputDecoration(
                      labelText: 'Description',
                      prefixIcon: const Icon(Icons.description),
                    ),
                    controller: descriptionController,
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: () {
                      final description = descriptionController.text;
                      if (description.isNotEmpty) {
                        context.read<HomeBloc>().add(
                          AddDescription(description),
                        );
                      } else {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Description cannot be empty'),
                          ),
                        );
                      }
                    },
                    child: const Text('Submit'),
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: () => descriptionController.clear(),
                    child: const Text('Clear'),
                  ),
                  const SizedBox(height: 20),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

  // @override
  // Widget build(BuildContext context) {
  //   return Scaffold(
  //     appBar: AppBar(title: Text('Home')),
  //     body: BlocProvider(
  //       create: (context) => HomeBloc(HomeRepository()),
  //       child: BlocListener<HomeBloc, HomeState>(
  //         listener: (context, state) {
  //           if (state is HomeLoaded) {
  //             ScaffoldMessenger.of(context).showSnackBar(
  //               const SnackBar(content: Text('Description added successfully')),
  //             );
  //           }
  //         },
  //         child: Center(
  //           child: Container(
  //             padding: const EdgeInsets.all(20),
  //             child: Column(
  //               mainAxisAlignment: MainAxisAlignment.center,
  //               children: <Widget>[
  //                 TextFormField(
  //                   decoration: InputDecoration(
  //                     labelText: 'Description',
  //                     prefixIcon: const Icon(Icons.description),
  //                   ),
  //                   controller: descriptionController,
  //                 ),
  //                 const SizedBox(height: 20),
  //                 ElevatedButton(
  //                   onPressed: () {
  //                     final description = descriptionController.text;
  //                     if (description.isNotEmpty) {
  //                       context.read<HomeBloc>().add(
  //                         AddDescription(description),
  //                       );
  //                     } else {
  //                       ScaffoldMessenger.of(context).showSnackBar(
  //                         const SnackBar(
  //                           content: Text('Description cannot be empty'),
  //                         ),
  //                       );
  //                     }
  //                   },
  //                   child: const Text('Submit'),
  //                 ),
  //                 const SizedBox(height: 20),
  //                 ElevatedButton(
  //                   onPressed: () => descriptionController.clear(),
  //                   child: const Text('Clear'),
  //                 ),
  //                 const SizedBox(height: 20),
  //               ],
  //             ),
  //           ),
  //         ),
  //       ),
  //     ),
  //   );
  // }
}
