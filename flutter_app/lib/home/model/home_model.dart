class HomeModel {
  final int id;
  final String title;
  final String description;
  final bool status;

  HomeModel({
    required this.id,
    required this.title,
    required this.description,
    required this.status,
  });

  factory HomeModel.fromJson(Map<String, dynamic> json) {
    return HomeModel(
      id: json['id'],
      title: json['title'],
      description: json['description'],
      status: json['status'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'description': description,
      'status': status,
    };
  }

  List<HomeModel> fromJsonList(List list) {
    return list.map((item) => HomeModel.fromJson(item)).toList();
  }
}
