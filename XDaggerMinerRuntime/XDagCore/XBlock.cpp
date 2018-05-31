#include "XBlock.h"
#include "XTime.h"

void XBlock::GenerateFakeBlock(cheatcoin_block *block)
{
    memset(block, 0, sizeof(struct cheatcoin_block));
    block->field[0].type = (int)cheatcoin_field_type::CHEATCOIN_FIELD_HEAD | (uint64_t)((int)cheatcoin_field_type::CHEATCOIN_FIELD_SIGN_OUT * 0x11) << 4;
    block->field[0].time = get_timestamp();
    block->field[0].amount = 0;
    block->field[0].transport_header = 1;
}
