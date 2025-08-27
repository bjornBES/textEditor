#include "Grammar.hpp"

#include <fstream>
#include <stdexcept>

using json = nlohmann::json;

void from_json(const json &j, Capture &c)
{
    j.at("scope").get_to(c.scope);
}

void to_json(json &j, const Capture &c)
{
    j = json{{"scope", c.scope}};
}

void from_json(const json &j, Pattern &p)
{
    if (j.contains("name"))
        p.name = j.at("name").get<std::string>();
    if (j.contains("include"))
        p.include = j.at("include").get<std::string>();
    if (j.contains("match"))
        p.match = j.at("match").get<std::string>();
    if (j.contains("begin"))
        p.begin = j.at("begin").get<std::string>();
    if (j.contains("end"))
        p.end = j.at("end").get<std::string>();
    if (j.contains("scope"))
        p.scope = j.at("scope").get<std::string>();

    if (j.contains("beginCaptures"))
        p.beginCaptures = j.at("beginCaptures").get<std::unordered_map<std::string, Capture>>();

    if (j.contains("endCaptures"))
        p.endCaptures = j.at("endCaptures").get<std::unordered_map<std::string, Capture>>();

    if (j.contains("patterns"))
        p.patterns = j.at("patterns").get<std::vector<Pattern>>();
}

void to_json(json &j, const Pattern &p)
{
    if (p.name)
        j["name"] = *p.name;
    if (p.include)
        j["include"] = *p.include;
    if (p.match)
        j["match"] = *p.match;
    if (p.begin)
        j["begin"] = *p.begin;
    if (p.end)
        j["end"] = *p.end;
    if (p.scope)
        j["scope"] = *p.scope;
    if (p.beginCaptures)
        j["beginCaptures"] = *p.beginCaptures;
    if (p.endCaptures)
        j["endCaptures"] = *p.endCaptures;
    if (p.patterns)
        j["patterns"] = *p.patterns;
}

void from_json(const json &j, Grammar &g)
{
    j.at("scopeName").get_to(g.scopeName);
    j.at("patterns").get_to(g.patterns);
    j.at("repository").get_to(g.repository);
}

void to_json(json &j, const Grammar &g)
{
    j = json{
        {"scopeName", g.scopeName},
        {"patterns", g.patterns},
        {"repository", g.repository}};
}

void GetGrammar(const std::string &grammarFile, Grammar *out)
{
    std::ifstream in(grammarFile);
    if (!in.is_open())
    {
        throw std::runtime_error("Failed to open grammar file: " + grammarFile);
    }

    json j;
    in >> j;
    *out = j.get<Grammar>();
}
