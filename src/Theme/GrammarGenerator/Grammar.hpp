#pragma once

#include <string>
#include <vector>
#include <unordered_map>
#include <optional>
#include <json.hpp>

struct Capture {
    std::string scope;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(Capture, scope)

struct Pattern {
    std::optional<std::string> name;
    std::optional<std::string> include;
    std::optional<std::string> match;
    std::optional<std::string> begin;
    std::optional<std::string> end;
    std::optional<std::string> scope;

    std::optional<std::unordered_map<std::string, Capture>> beginCaptures;
    std::optional<std::unordered_map<std::string, Capture>> endCaptures;
    std::optional<std::vector<Pattern>> patterns;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(Pattern, name, include, match, begin, end, scope, beginCaptures, endCaptures, patterns)

using Repository = std::unordered_map<std::string, Pattern>;

struct Grammar {
    std::string scopeName;
    std::vector<Pattern> patterns;
    Repository repository;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(Grammar, scopeName, patterns, repository)

void GetGrammar(const std::string& grammarFile, Grammar* out);
