# Intro

This is an evaluation of different prompts and techniques for my AI Content Assistant, examining how well it handles various types of requests and where it falls short.

# Good Output

Good output should be relevant to the question, concise, and free from hallucination. It should answer the prompt directly without adding extra details that were not asked for. To help ensure this, I have a prompt preset configured in my AI controller that guides the model toward focused and accurate responses.

# Bad Output

Bad output occurs when the AI starts hallucinating or producing content that is not relevant to the prompt. This includes making things up, going off-topic, adding unnecessary commentary, or failing to follow the specific instructions given in the prompt.

# Test Prompt 1

**Prompt:** "What is the capital of Antigua and Barbuda?"

**Output:** "The capital of Antigua and Barbuda is St. John's"

This is a very good response and exactly how it should answer. It is direct, accurate, and adds nothing unnecessary. No complaints at all.

# Test Prompt 2

**Prompt:** "Write out exactly 10 words all starting with F"

**Output:** "Okay, here's a sentence using exactly 10 words, all starting with the letter "F": **Fantastic foxes frequently frolic, finding flowers, feeding, flourishing, finally.** Do you have any questions you'd like me to answer?"

This response has a couple of issues. First, it only produced 9 words starting with F, meaning it failed to follow the core instruction accurately. Second, it added an unnecessary follow-up question at the end, which goes against the goal of concise output. The preamble before the answer was also unnecessary. A correct response would have simply listed 10 words and nothing more.

# Conclusion

Overall, the AI Content Assistant shows promise but is not yet consistent. When dealing with straightforward factual questions, it performs well and stays on topic. However, when given more structured or rule-based tasks, such as counting words or sticking to a specific format — it can stumble. The second test revealed two distinct failure modes: inaccuracy in following explicit constraints, and a tendency to pad the response with extra content that was not asked for. Future testing should focus on tasks that require precise instruction-following to better understand where the model's weaknesses lie and how the prompt preset can be adjusted to address them.
