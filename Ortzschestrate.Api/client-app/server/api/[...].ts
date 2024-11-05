export default defineEventHandler(async (event) => {
  console.log("inside event");
  const proxyUrl = process.env.NUXT_API_URL;
  console.log("proxyUrl: ", proxyUrl);
  const target = proxyUrl + event.path;
  console.log("target: ", target);

  try {
    const res = await proxyRequest(event, target);
    console.log("res: ", res);
    return res;
  } catch (e) {
    console.log("proxy error: ", e);
  }
});
