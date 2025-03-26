class Usermodel {
  final String username;
  final String password;
  final String accessToken;

  Usermodel({required this.username, required this.password, this.accessToken = ''});

  factory Usermodel.fromJson(Map<String, dynamic> json) {
    return Usermodel(
      username: json['username'],
      password: json['password'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'username': username,
      'password': password,
    };
  }
}